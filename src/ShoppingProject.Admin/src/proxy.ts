// Proxy configuration for API requests to .NET backend
// Rename this file to `proxy.ts` to enable it.
import { type NextRequest, NextResponse } from "next/server";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

/**
 * Runs before requests complete.
 * Use for rewrites, redirects, or header changes.
 * Proxies API requests to the .NET backend
 */
export function proxy(req: NextRequest) {
  const url = req.nextUrl;

  // Only proxy API requests
  if (url.pathname.startsWith("/api/")) {
    // Clone the request to avoid consuming the body
    const targetUrl = `${API_BASE_URL}${url.pathname}${url.search}`;

    return fetch(targetUrl, {
      method: req.method,
      headers: {
        ...Object.fromEntries(req.headers.entries()),
        // Forward authorization header
        "Authorization": req.headers.get("Authorization") || "",
      },
      body: req.method !== "GET" && req.method !== "HEAD" ? req.body : undefined,
      redirect: "manual",
    }).then((response) => {
      return new NextResponse(response.body, {
        status: response.status,
        statusText: response.statusText,
        headers: new Headers(response.headers),
      });
    });
  }

  return NextResponse.next();
}

/**
 * Matcher runs for all routes.
 * To skip assets or APIs, use a negative matcher from docs.
 */
export const config = {
  matcher: "/:path*",
};
