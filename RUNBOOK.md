# Runbook

## Hotfix flow

For an urgent fix that cannot wait for the normal `feature → develop → main` promotion: branch from `main` as `hotfix/<short-description>`, push commits, then run the **Deploy to Test** workflow manually against the hotfix branch to validate against `test-identity.smol.la`. Once QA signs off, open a PR targeting `main`; branch protection still applies (PR + green `test` check + admin-enforced + no force-push). Merging the PR triggers **Deploy Production**, which ships `:latest` to `identity.smol.la`. The **Sync main into develop** workflow then opens a back-merge PR automatically — **merging that PR is mandatory**: without it, the next normal `develop → main` promotion will silently revert the hotfix on prod.
