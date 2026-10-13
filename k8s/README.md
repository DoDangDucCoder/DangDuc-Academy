# Kubernetes Deployment

This base deploys the Phase 5 scale stack:

- backend API
- video API
- frontend
- admin dashboard
- PostgreSQL
- Redis
- Elasticsearch
- Nginx ingress routes
- autoscaling for API workloads

## Render Manifests

```powershell
kubectl kustomize .\k8s\base
```

## Deploy

Create the runtime secret first. You can copy `k8s/base/secret.example.yaml` to an environment-specific private manifest, or create the secret directly:

```powershell
kubectl create namespace hdnx-udemy
kubectl -n hdnx-udemy create secret generic hdnx-secrets `
  --from-literal=POSTGRES_PASSWORD="<secure-password>" `
  --from-literal=REDIS_PASSWORD="" `
  --from-literal=JWT_SECRET="<32-byte-minimum-secret>"
kubectl apply -k .\k8s\base
```

Replace the placeholder image names in `backend.yaml`, `video-api.yaml`, `frontend.yaml`, and `admin.yaml` with your registry paths before deploying outside local clusters.

## Environment Overlays

```powershell
kubectl kustomize .\k8s\overlays\staging
kubectl kustomize .\k8s\overlays\production
```

The overlays set environment-specific image tags, replica counts, and TLS placeholders for cert-manager. Keep real secrets outside this repository and create `hdnx-secrets` per namespace before applying either overlay.

## Staging Checklist

Before applying staging, make sure these values are prepared outside the repository:

- GHCR images exist for `backend-api:staging`, `video-api:staging`, `frontend:staging`, and `admin:staging`.
- `hdnx-secrets` exists in the target namespace with database, Redis, and JWT values.
- Ingress hosts in `k8s/overlays/staging/ingress.yaml` are changed from `example.com` to real staging domains.
- `cert-manager`, the Nginx ingress controller, and a `letsencrypt-staging` ClusterIssuer are installed in the cluster.

Apply staging:

```powershell
kubectl apply -k .\k8s\overlays\staging
```

Smoke test after rollout:

```powershell
kubectl -n hdnx-udemy get pods
kubectl -n hdnx-udemy rollout status deployment/staging-backend
kubectl -n hdnx-udemy rollout status deployment/staging-video-api
kubectl -n hdnx-udemy get ingress
```

Then verify the public staging URLs:

- frontend loads and can call `/api/v1/master-course/course/search/1/20`
- admin loads and can sign in
- backend `/health/live` and `/health/ready` return healthy
- video API `/health/live` and `/health/ready` return healthy
- Elasticsearch-backed search works, and DB fallback still returns courses if Elasticsearch is unavailable

Use the production overlay only after the same checks pass on staging and the production hostnames, TLS issuer, image tags, and secrets are ready.
