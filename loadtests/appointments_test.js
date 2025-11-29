import http from "k6/http";
import { sleep } from "k6";

export const options = {
    stages: [
        { duration: "10s", target: 5 },
        { duration: "30s", target: 10 },
        { duration: "10s", target: 0 },
    ],
    thresholds: {
        http_req_duration: ["p(95)<1000"],
        http_req_failed: ["rate<0.05"],
    },
};

const API_BASE = "https://chair-api.onrender.com";
const TEST_USER_ID = "2cd692eb-d4b8-4c1c-8026-c3677754bf30";

export default function () {
    const payload = JSON.stringify({
        userId: TEST_USER_ID,
        serviceId: "0de06511-aa1e-422a-8abc-97cf323cd9e0",
        stylistId: "83c3b32c-bfd8-44c5-b61d-0fbe747d099c",
        startTime: new Date().toISOString(),
        endTime: new Date().toISOString(),
        status: 0,
    });

    const params = { headers: { "Content-Type": "application/json" } };
    http.post(`${API_BASE}/api/appointments`, payload, params);
    sleep(Math.random() * 3);
}

export function teardown(data) {
    console.log("Cleaning up test data...");
    const delRes = http.del(`${API_BASE}/api/cleanup-test-user`);
    console.log(`Cleanup response: ${delRes.status} - ${delRes.body}`);
}
