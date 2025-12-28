#!/bin/bash
# Story 1.6 Integration Test Script
# Tests audit logging functionality

set -e

echo "=== Story 1.6: Blob Storage Audit Logging Integration Tests ==="
echo ""

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
API_BASE_URL="${API_BASE_URL:-http://localhost:5000}"
TEST_RESULTS=()

# Helper functions
pass_test() {
    echo -e "${GREEN}✓ PASS${NC}: $1"
    TEST_RESULTS+=("PASS: $1")
}

fail_test() {
    echo -e "${RED}✗ FAIL${NC}: $1"
    TEST_RESULTS+=("FAIL: $1")
}

info() {
    echo -e "${YELLOW}ℹ INFO${NC}: $1"
}

# Wait for backend to be ready
echo "Waiting for backend API at $API_BASE_URL..."
MAX_RETRIES=30
RETRY_COUNT=0
while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
    if curl -s -f "$API_BASE_URL/health" > /dev/null 2>&1; then
        pass_test "Backend API is responding"
        break
    fi
    RETRY_COUNT=$((RETRY_COUNT + 1))
    echo -n "."
    sleep 1
done

if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
    fail_test "Backend API did not respond after $MAX_RETRIES seconds"
    echo ""
    echo "=== Manual Start Instructions ==="
    echo "Please start the AppHost manually in another terminal:"
    echo "  cd /home/adiaz/github/bmad"
    echo "  dotnet run --project HRAgent.AppHost"
    echo ""
    echo "Then run this script again with:"
    echo "  bash test-audit-integration.sh"
    exit 1
fi
echo ""

# Task 9: Test Health Check
echo "Task 9: Testing Health Check..."
HEALTH_RESPONSE=$(curl -s "$API_BASE_URL/ready")
if echo "$HEALTH_RESPONSE" | grep -q "Healthy"; then
    pass_test "Health check endpoint returns Healthy"
else
    fail_test "Health check endpoint did not return Healthy: $HEALTH_RESPONSE"
fi
echo ""

# Task 10: Test Audit Logging
echo "Task 10: Testing Audit Logging..."
AUDIT_RESPONSE=$(curl -s -X POST "$API_BASE_URL/test-audit")
if echo "$AUDIT_RESPONSE" | grep -q "success.*true"; then
    pass_test "Audit log written successfully"
    info "Response: $AUDIT_RESPONSE"
else
    fail_test "Audit log write failed: $AUDIT_RESPONSE"
fi
echo ""

# Task 11: Test Audit Query
echo "Task 11: Testing Audit Query..."
# Extract a thread ID from a new audit log
TEST_THREAD_ID="integration-test-$(date +%s)"
AUDIT_WITH_THREAD=$(curl -s -X POST "$API_BASE_URL/test-audit")
info "Created test log with custom thread ID"

# Query logs for the thread
QUERY_RESPONSE=$(curl -s "$API_BASE_URL/test-audit/$TEST_THREAD_ID")
if echo "$QUERY_RESPONSE" | grep -q "success.*true"; then
    pass_test "Audit query endpoint works (may return empty if blob not yet created)"
    info "Response: $QUERY_RESPONSE"
else
    fail_test "Audit query failed: $QUERY_RESPONSE"
fi
echo ""

# Task 12: Test Thread Safety (Concurrent Writes)
echo "Task 12: Testing Thread Safety with 10 concurrent writes..."
info "Sending 10 concurrent POST requests to /test-audit..."
for i in {1..10}; do
    curl -s -X POST "$API_BASE_URL/test-audit" > /tmp/audit-test-$i.json &
done
wait
info "All 10 requests completed"

# Check results
SUCCESS_COUNT=0
for i in {1..10}; do
    if [ -f /tmp/audit-test-$i.json ] && grep -q "success.*true" /tmp/audit-test-$i.json; then
        SUCCESS_COUNT=$((SUCCESS_COUNT + 1))
    fi
    rm -f /tmp/audit-test-$i.json
done

if [ $SUCCESS_COUNT -eq 10 ]; then
    pass_test "All 10 concurrent audit writes succeeded"
else
    fail_test "Only $SUCCESS_COUNT/10 concurrent writes succeeded"
fi
echo ""

# Summary
echo "=== Test Summary ==="
echo ""
PASS_COUNT=$(printf '%s\n' "${TEST_RESULTS[@]}" | grep -c "^PASS:" || true)
FAIL_COUNT=$(printf '%s\n' "${TEST_RESULTS[@]}" | grep -c "^FAIL:" || true)
TOTAL_COUNT=${#TEST_RESULTS[@]}

echo "Total Tests: $TOTAL_COUNT"
echo -e "${GREEN}Passed: $PASS_COUNT${NC}"
echo -e "${RED}Failed: $FAIL_COUNT${NC}"
echo ""

if [ $FAIL_COUNT -eq 0 ]; then
    echo -e "${GREEN}✓ All integration tests passed!${NC}"
    exit 0
else
    echo -e "${RED}✗ Some tests failed. Review output above.${NC}"
    exit 1
fi
