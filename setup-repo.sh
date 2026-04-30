#!/usr/bin/env bash
#
# One-shot bootstrap for repository-side configuration:
#   - Branch protection on `main`    (require PR + green CI, no force push)
#   - Branch protection on `develop` (require green CI)
#
# Both `main` and `develop` must already exist on the remote when this is run,
# because protection rules cannot be applied to branches that do not exist.
#
# Requires: gh CLI authenticated (`gh auth status`).
# Usage:    ./setup-repo.sh
#
# Notes on the API:
#   - `restrictions` and `required_pull_request_reviews` accept null OR an
#     object. The `gh api -F` form encoding cannot send a literal null, so we
#     build a JSON body and PUT it via --input -.
#   - This call is idempotent: PUTting the same body again is a no-op.
#

set -euo pipefail

REPO="adamsalisbury/smolla-identity-provider"
STATUS_CHECK="test"

echo "Configuring branch protection for ${REPO}"

if ! command -v gh >/dev/null 2>&1; then
    echo "error: gh CLI is not installed" >&2
    exit 1
fi

if ! gh auth status >/dev/null 2>&1; then
    echo "error: gh CLI is not authenticated; run 'gh auth login'" >&2
    exit 1
fi

protect_branch() {
    local branch="$1"
    local enforce_admins="$2"
    local require_pr="$3"

    echo "  -> protecting ${branch}"

    local pr_block="null"
    if [[ "${require_pr}" == "true" ]]; then
        pr_block='{"required_approving_review_count":0,"dismiss_stale_reviews":true,"require_code_owner_reviews":false}'
    fi

    local body
    body=$(cat <<EOF
{
    "required_status_checks": {
        "strict": true,
        "contexts": ["${STATUS_CHECK}"]
    },
    "enforce_admins": ${enforce_admins},
    "required_pull_request_reviews": ${pr_block},
    "restrictions": null,
    "allow_force_pushes": false,
    "allow_deletions": false,
    "required_linear_history": false,
    "required_conversation_resolution": false
}
EOF
)

    echo "${body}" | gh api -X PUT "repos/${REPO}/branches/${branch}/protection" --input - >/dev/null
}

# main:    require PR + green CI, enforce on admins
# develop: require green CI only (you push direct as a solo dev)
protect_branch "main" "true" "true"
protect_branch "develop" "false" "false"

echo "Done."
