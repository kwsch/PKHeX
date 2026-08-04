#!/usr/bin/env bash
#
# Generate a self-signed CODE SIGNING identity as a .p12 (tertius pattern).
#
# This .p12 is THE signing identity. Reusing the same one for every release
# keeps the codesign designated requirement stable, so macOS treats upgrades
# as the same app — keychain items and any TCC (Accessibility, Full Disk
# Access, etc.) grants persist across updates instead of re-prompting the
# user every time. Store it as a GitHub Actions secret for CI.
#
# Usage:   P12_PASSWORD=... Scripts/make-signing-cert.sh [output-dir]
# Env:     P12_PASSWORD (required)   CERT_CN (default "Patrik Lleshaj")
#
set -euo pipefail

OUTDIR="${1:-./secrets}"
CN="${CERT_CN:-Patrik Lleshaj}"
: "${P12_PASSWORD:?set P12_PASSWORD}"

mkdir -p "$OUTDIR"
WORK="$(mktemp -d)"
trap 'rm -rf "$WORK"' EXIT

cat > "$WORK/ext.cnf" <<EOF
[req]
distinguished_name = dn
prompt = no
x509_extensions = v3
[dn]
CN = $CN
[v3]
basicConstraints = critical,CA:false
keyUsage = critical,digitalSignature
extendedKeyUsage = critical,codeSigning
subjectKeyIdentifier = hash
EOF

# EC key + self-signed cert with the codeSigning EKU.
openssl ecparam -name prime256v1 -genkey -noout -out "$WORK/key.pem"
openssl req -new -x509 -key "$WORK/key.pem" -out "$WORK/cert.pem" -days 3650 -config "$WORK/ext.cnf"

# Package as .p12. System openssl is LibreSSL — do NOT pass -legacy.
openssl pkcs12 -export \
  -inkey "$WORK/key.pem" -in "$WORK/cert.pem" \
  -out "$OUTDIR/signing.p12" -name "$CN" \
  -passout pass:"$P12_PASSWORD"

echo "Wrote $OUTDIR/signing.p12  (CN: $CN)"
echo "For CI:   base64 -i $OUTDIR/signing.p12 | pbcopy   → store as SELFSIGN_CERT_P12_BASE64"
