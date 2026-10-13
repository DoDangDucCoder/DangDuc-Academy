# Cloudflare CDN Cache Rules

Use these Phase 5 rules as a starting point for the frontend, admin dashboard, and media delivery domains.

## Static Angular Assets

Match:

```text
(http.host eq "web-hdnx.example.com" or http.host eq "admin-hdnx.example.com")
and http.request.uri.path matches "^/.*\\.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$"
```

Settings:

- Cache eligibility: eligible for cache
- Edge TTL: 1 month
- Browser TTL: 1 year
- Cache key: host + path + query string

## HTML Shell

Match:

```text
(http.host eq "web-hdnx.example.com" or http.host eq "admin-hdnx.example.com")
and not http.request.uri.path matches "^/api/.*"
and not http.request.uri.path matches "^/.*\\.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$"
```

Settings:

- Cache eligibility: bypass cache
- Browser TTL: respect origin

## API And Auth

Match:

```text
http.request.uri.path matches "^/api/.*"
or http.request.uri.path contains "/hub-notification"
```

Settings:

- Cache eligibility: bypass cache
- Forward all headers required by auth and SignalR

## Video Segments

Match:

```text
http.host eq "video-hdnx.example.com"
and http.request.uri.path matches "^/.*\\.(m3u8|ts|mp4)$"
```

Settings:

- Cache eligibility: eligible for cache
- Edge TTL: 1 day for `.m3u8`, 7 days for `.ts` and `.mp4`
- Browser TTL: 10 minutes for `.m3u8`, 1 day for `.ts` and `.mp4`

Keep signed URLs or token checks at the origin for private course content. Public previews can use longer TTLs.

## Deployment Checklist

- Confirm Nginx serves static Angular assets with `Cache-Control: public, immutable`.
- Confirm SPA HTML routes return `Cache-Control: no-store, no-cache, must-revalidate`.
- Confirm `/api/*` and `/hub-notification` return `Cache-Control: no-store`.
- Purge CDN cache after each frontend/admin deployment if asset hashing is disabled.
- Configure Google OAuth, Google Maps, Firebase, and Stripe publishable keys via `docs/environment-keys.md`; do not put real keys in source.
