// Proxy configuration for API requests to .NET backend
// and route protection for authenticated pages
import { type NextRequest, NextResponse } from "next/server";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

// Routes that require authentication
const protectedRoutes = ["/dashboard", "/admin"];

// Routes that should redirect to dashboard if already authenticated
const authRoutes = ["/auth/v1/login", "/auth/v1/register", "/auth/v2/login", "/auth/v2/register"];

/**
 * Runs before requests complete.
 * Use for rewrites, redirects, or header changes.
 * Proxies API requests to the .NET backend
 * Protects authenticated routes
 */
export function proxy(req: NextRequest) {
  const url = req.nextUrl;
  const pathname = url.pathname;

  // Check for token - check both cookie and localStorage via header
  const token = req.cookies.get("access_token")?.value;
  const isAuthenticated = !!token;

  // Check if it's a protected route
  const isProtectedRoute = protectedRoutes.some((route) => pathname.startsWith(route));
  const isAuthRoute = authRoutes.includes(pathname);

  // If trying to access protected route without auth, redirect to login
  if (isProtectedRoute && !isAuthenticated) {
    const loginUrl = new URL("/auth/v2/login", req.url);
    loginUrl.searchParams.set("redirect", pathname);
    return NextResponse.redirect(loginUrl);
  }

  // If already authenticated and trying to access auth routes, redirect to dashboard
  if (isAuthRoute && isAuthenticated) {
    return NextResponse.redirect(new URL("/dashboard/default", req.url));
  }

  // Only proxy API requests
  if (pathname.startsWith("/api/")) {
    // Clone the request to avoid consuming the body
    const targetUrl = `${API_BASE_URL}${pathname}${url.search}`;

    return fetch(targetUrl, {
      method: req.method,
      headers: {
        ...Object.fromEntries(req.headers.entries()),
        // Forward authorization header
        Authorization: req.headers.get("Authorization") || "",
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
