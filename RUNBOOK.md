# Runbook

## Standard promotion flow

Day-to-day work flows `feature/<description>` → `develop` → `main`. Branch from `develop`, push commits, then use the **Deploy to Test** workflow manually against the feature branch to validate against `test-identity.smol.la` while QA reviews. When QA signs off, open a PR targeting `develop`; CI must pass, then merge — `Deploy Staging` ships `:staging` to `staging-identity.smol.la` automatically. Soak on staging, then open a PR `develop → main`. Merging that PR (PR + green `test` + admin-enforced + no force-push) triggers `Deploy Production` and ships `:latest` to `identity.smol.la`.

## Hotfix flow

For an urgent fix that cannot wait for the normal `feature → develop → main` promotion: branch from `main` as `hotfix/<short-description>`, push commits, then run the **Deploy to Test** workflow manually against the hotfix branch to validate against `test-identity.smol.la`. Once QA signs off, open a PR targeting `main`; branch protection still applies (PR + green `test` check + admin-enforced + no force-push). Merging the PR triggers **Deploy Production**, which ships `:latest` to `identity.smol.la`. The **Sync main into develop** workflow then opens a back-merge PR automatically — **merging that PR is mandatory**: without it, the next normal `develop → main` promotion will silently revert the hotfix on prod.
