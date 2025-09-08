# Trivy Security Scanning Test

This file is used to test the Trivy security scanning functionality in the CI pipeline.

## Test Cases

### Secret Detection Test
The following commented lines contain test secrets that Trivy should detect:

```bash
# export API_KEY="sk-1234567890abcdef1234567890abcdef"  # Fake API key for testing
# DATABASE_PASSWORD="super_secret_password_123"  # Fake password for testing
```

### Vulnerability Detection Test
The Trivy filesystem scanner will check for:
- Known vulnerable dependencies in project files
- Configuration issues
- Security misconfigurations

## Expected Behavior

When the CI pipeline runs:
1. **Secret scan** should detect the commented test secrets above
2. **Filesystem scan** should check all project dependencies
3. **Container scan** should validate the Docker image (when built)
4. **Pipeline** should fail if critical vulnerabilities are found

## Manual Testing

To test locally, install Trivy and run:

```bash
# Install Trivy
curl -sfL https://raw.githubusercontent.com/aquasecurity/trivy/main/contrib/install.sh | sh -s -- -b /usr/local/bin

# Run filesystem scan
trivy fs --severity CRITICAL,HIGH,MEDIUM .

# Run secret scan
trivy fs --scanners secret .

# Run configuration scan
trivy fs --scanners config .
```

## Notes

- This file is for testing purposes and should be kept in the repository
- Real secrets should never be committed to version control
- The CI pipeline will scan this file and report any findings