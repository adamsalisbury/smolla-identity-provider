#!/usr/bin/env bash
#
# One-shot bootstrap for repository-side configuration:
#   - Branch protection on `main` (require PR + green CI, no force push)
#   - Branch protection on `develop` (require green CI)
#
# Both `main` and `develop` must already exist on the remote when this is run,
# because protection rules cannot be applied to branches that do not exist.
#
# Requires: gh CLI authenticated (`gh auth status`).
# Usage:    ./setup-repo.sh
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
    local require_pr_review_count="$2"

    echo "  -> protecting ${branch}"

    gh api -X PUT "repos/${REPO}/branches/${branch}/protection" \
        -F "required_status_checks[strict]=true" \
        -F "required_status_checks[contexts][]=${STATUS_CHECK}" \
        -F "enforce_admins=true" \
        -F "required_pull_request_reviews[required_approving_review_count]=${require_pr_review_count}" \
        -F "required_pull_request_reviews[dismiss_stale_reviews]=true" \
        -F "restrictions=" \
        -F "allow_force_pushes=false" \
        -F "allow_deletions=false"
}

protect_branch "main" 0
protect_branch "develop" 0

echo "Done."
