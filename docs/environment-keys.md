# Environment Keys

Các key public/legacy cũ đã được xoá khỏi source. Khi thay key mới, không commit giá trị thật vào repo.

## Keys Removed From Source

- Frontend Google OAuth client id
- Frontend Facebook app id
- Frontend Stripe publishable key
- Admin Firebase config
- Admin Google Maps key
- Admin Stripe publishable key

## Required Variables For New Keys

Use these variable names in local `.env`, CI/CD secrets, Kubernetes secrets/config, or deployment-time config:

```env
FRONTEND_GOOGLE_CLIENT_ID=
FRONTEND_FACEBOOK_APP_ID=
FRONTEND_STRIPE_PUBLISHABLE_KEY=
ADMIN_FIREBASE_API_KEY=
ADMIN_FIREBASE_AUTH_DOMAIN=
ADMIN_FIREBASE_DATABASE_URL=
ADMIN_FIREBASE_PROJECT_ID=
ADMIN_FIREBASE_STORAGE_BUCKET=
ADMIN_FIREBASE_MESSAGING_SENDER_ID=
ADMIN_FIREBASE_APP_ID=
ADMIN_FIREBASE_MEASUREMENT_ID=
ADMIN_GOOGLE_MAPS_API_KEY=
ADMIN_STRIPE_PUBLISHABLE_KEY=
```

## Local Setup

For local development, copy `.env.example` to `.env` and fill only the keys needed for the feature being tested.

Angular still reads the existing `environment` property names for build compatibility:

- `environment.clientIdGoogle`
- `environment.facebookId`
- `environment.stripePublicKey`
- `environment.firebaseConfig`
- `environment.googleMapsApiKey`

## Staging And Production

- Store real values in CI/CD secrets, host environment variables, or Kubernetes secrets/config.
- Do not put real values into `environment.prod.ts`.
- Do not store backend secrets in frontend/admin config.
- Use separate keys per environment and rotate them when moving to production.

## Notes

Google Maps is not loaded from `admin/src/index.html`. If Maps is enabled again, load it using `ADMIN_GOOGLE_MAPS_API_KEY` during deployment/config generation.
