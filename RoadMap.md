# Mini Wallet System — Phased Roadmap

## Phase 1

**Goal:** Build a working closed-loop wallet with proper authentication, transaction integrity, and basic secret management.

| #  | Feature                                                                 |
| -- | ----------------------------------------------------------------------- |
| 1  | User auth (register, email verification, set password, login, JWT)      |                                |
| 2  | Wallet creation + balance                                               |
| 3  | Top-up (single payment method)                                          |
| 4  | P2P transfer between users                                              |
| 5  | Transaction history (immutable ledger)                                  |
| 6  | Idempotency keys                                                        |
| 7  | Basic transaction limits (per-txn max)                                  |
| 8  | Secret Vault integration (OpenBao)                                      |
| 9  | Secure application configuration                                        |
| 10 | Environment-specific configuration (Development / Staging / Production) |

### Secret Vault Scope

OpenBao should manage sensitive configuration such as:

* JWT signing key
* JWT issuer/audience where appropriate
* Database connection string
* SMTP credentials (host, port, username, password/API key used to send verification emails)
* External payment-provider credentials
* API keys
* Encryption keys
* Other application secrets

Application configuration should follow this general flow:

```text
Docker / Environment
        │
        ▼
   OpenBao Config
        │
        ▼
     OpenBao
        │
        │ secrets
        ▼
   ASP.NET Core
        │
        ├── JWT
        ├── Database
        ├── SMTP (email verification)
        └── External Services
```

The application should **not** commit secrets into:

```text
appsettings.json
appsettings.Production.json
.env
source control
Dockerfile
GitHub repository
```

Non-sensitive configuration can remain in normal ASP.NET Core configuration.

### Phase 1 Exit Criteria

Two users can:

1. Register with an email address
2. Receive a verification email (sent via SMTP, credentials pulled from OpenBao) containing a token
3. Use the token to set their password and activate their account
4. Login and receive a JWT (only after verification)
5. Create/use a wallet
6. Top up their wallet
7. Transfer money to another user
8. View transaction history
9. Maintain a correct balance backed by the ledger
10. Prevent duplicate transactions using idempotency keys
11. Enforce basic transaction limits
12. Start the API with JWT/database/SMTP secrets retrieved from OpenBao rather than hard-coded secrets

---

## Phase 2

**Goal:** Make the wallet capable of handling realistic payment workflows and operational scenarios.

| #  | Feature                                   |
| -- | ----------------------------------------- |
| 11 | External payouts (withdraw to bank/card)  |
| 12 | Fee engine (configurable per txn type)    |
| 13 | Dispute / chargeback states               |
| 14 | Notifications (email + push)              |
| 15 | Account freeze / unfreeze (admin)         |
| 16 | Basic fraud rules (velocity, geo, device) |
| 17 | Admin dashboard (search, view, adjust)    |
| 18 | Audit logging                             |
| 19 | Health checks and readiness checks        |
| 20 | Structured logging + centralized logs     |

### Exit Criteria

A user can withdraw to a real bank account; an admin can freeze a suspicious wallet; a failed payout is handled gracefully; and operators can identify application and transaction problems through logs and health checks.

---

## Phase 3

**Goal:** Support real business use cases and external integrations.

| #  | Feature                                                    |
| -- | ---------------------------------------------------------- |
| 21 | Webhooks / public API                                      |
| 22 | Recurring / scheduled payments                             |
| 23 | Split payments (1 → N)                                     |
| 24 | Multi-wallet per user (savings, spending)                  |
| 25 | Payment request links (no full POS)                        |
| 26 | Reconciliation engine (auto-match vs. provider settlement) |
| 27 | API versioning                                             |
| 28 | Rate limiting                                              |
| 29 | External API authentication                                |
| 30 | Background job processing                                  |

### Exit Criteria

A third-party service can trigger a payment via API; a merchant can split a payout to multiple vendors; you can detect a $0.03 discrepancy in settlement; and external clients can safely consume versioned APIs.

---

## Phase 4 — Scale & Compliance (Month 4+)

**Goal:** Operate in regulated markets at volume.

| #  | Feature                                              |
| -- | ---------------------------------------------------- |
| 31 | KYC / AML pipeline (tiered)                          |
| 32 | Multi-currency + FX at ledger level                  |
| 33 | Virtual cards / payment tokens                       |
| 34 | Audit / compliance export (hash-chained logs)        |
| 35 | Multi-region support                                 |
| 36 | Advanced fraud detection                             |
| 37 | Key rotation and advanced secret-management policies |
| 38 | Disaster recovery / backup strategy                  |

### Exit Criteria

You can pass a basic regulatory audit; a user in a different currency can transact with correct FX applied at the ledger, not the UI; and the platform has appropriate operational controls for larger-scale deployment.

---

# Key Design Principles

1. **Ledger-first** — balance is a projection derived from the transaction log.

2. **Double-entry** — every debit has a matching credit.

3. **Immutable records** — transactions are append-only.

4. **Explicit state machine** —

```text
CREATED
   ↓
PENDING
   ├──→ COMPLETED
   ├──→ FAILED
   └──→ REVERSED
```

5. **Serialize wallet updates** — use optimistic locking or row-level locks to prevent concurrent balance corruption.

6. **Idempotency everywhere money moves** — retries must not create duplicate financial transactions.

7. **Secrets outside the application source code** — sensitive values should be retrieved from the secret-management system.

8. **Configuration ≠ secrets** — normal configuration belongs in `appsettings.json`; sensitive values belong in OpenBao.

9. **Fail fast on missing secrets** — the API should not start if critical secrets such as the JWT key or database connection string are unavailable.

10. **Never log secret values** — diagnostics may identify that a secret is missing, but must never print its actual value.

---

# Minimal Data Model

```sql
wallets (
    wallet_id       UUID PRIMARY KEY,
    user_id         UUID NOT NULL,
    currency        CHAR(3) NOT NULL,
    status          VARCHAR(20) NOT NULL, -- ACTIVE | FROZEN
    created_at      TIMESTAMP NOT NULL
)

transactions (
    transaction_id  UUID PRIMARY KEY,
    wallet_id       UUID NOT NULL,
    amount          DECIMAL(15,4) NOT NULL,
    direction       VARCHAR(10) NOT NULL, -- CREDIT | DEBIT
    status          VARCHAR(20) NOT NULL, -- CREATED | PENDING | COMPLETED |
                                          -- FAILED | REVERSED | DISPUTED
    reference_id    UUID NOT NULL UNIQUE, -- idempotency key
    counterparty_id UUID,
    fee             DECIMAL(15,4) DEFAULT 0,
    created_at      TIMESTAMP NOT NULL
)
```

---

# Phase 1 Architecture

```text
                         ┌─────────────────┐
                         │     OpenBao     │
                         │   Secret Vault  │
                         └────────┬────────┘
                                  │
                              Secrets
                                  │
                                  ▼
┌──────────────┐        ┌────────────────────┐
│   Client     │───────▶│   ASP.NET Core API │
└──────────────┘        └─────────┬──────────┘
                                  │
                    ┌─────────────┼─────────────┐
                    │             │             │
                    ▼             ▼             ▼
                 Auth/JWT     Services      Repositories
                                  │             │
                                  │             ▼
                                  │         SQL Server
                                  │
                                  ▼
                              Ledger
```

For your current project, this is a good Phase 1 target because you're not just building CRUD endpoints. You're demonstrating **authentication + financial transaction integrity + secret management + repository/service architecture + Docker + infrastructure awareness** in one coherent MVP.
