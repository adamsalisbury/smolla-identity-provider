#!/usr/bin/env bash
#
# Deploy the smolla-identity-provider Docker image to a target environment on
# the Linode. Invoked by the GitHub Actions deploy workflows. Because the
# self-hosted runner lives on the same Linode as the docker compose project,
# this script talks to the local Docker daemon directly — no SSH, no remote
# transport, no extra credentials.
#
# Usage:
#   ./deploy.sh test       # docker pull + docker compose up -d identity-test
#   ./deploy.sh staging    # docker pull + docker compose up -d identity-staging
#   ./deploy.sh prod       # docker pull + docker compose up -d identity-prod
#

set -euo pipefail

ENVIRONMENT="${1:-}"
COMPOSE_PROJECT_DIR="${SMOLLA_COMPOSE_DIR:-/opt/smolla/identity}"

case "${ENVIRONMENT}" in
    test)
        SERVICE="identity-test"
        IMAGE_TAG="test"
        ;;
    staging)
        SERVICE="identity-staging"
        IMAGE_TAG="staging"
        ;;
    prod)
        SERVICE="identity-prod"
        IMAGE_TAG="latest"
        ;;
    *)
        echo "usage: $0 {test|staging|prod}" >&2
        exit 1
        ;;
esac

if [[ ! -d "${COMPOSE_PROJECT_DIR}" ]]; then
    echo "error: compose project directory not found: ${COMPOSE_PROJECT_DIR}" >&2
    exit 1
fi

if ! command -v docker >/dev/null 2>&1; then
    echo "error: docker is not installed" >&2
    exit 1
fi

echo "Deploying ${SERVICE} (tag: ${IMAGE_TAG}) from ${COMPOSE_PROJECT_DIR}"

cd "${COMPOSE_PROJECT_DIR}"

docker compose pull "${SERVICE}"
docker compose up -d --no-deps --force-recreate "${SERVICE}"

# Prune the previous image only if there are dangling layers; keeps disk in
# check on the Linode without nuking other tags.
docker image prune --force --filter "label=org.opencontainers.image.source=https://github.com/adamsalisbury/smolla-identity-provider" >/dev/null

echo "Deployment complete: ${SERVICE}"
