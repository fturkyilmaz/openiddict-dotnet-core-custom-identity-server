"use client";

import CryptoJS from "crypto-js";

// Encryption key - in production, this should be derived from device fingerprint
// or fetched from a secure server. For now, we use a fixed key.
const ENCRYPTION_KEY = "shopping-admin-secure-key-v1";

/**
 * Encrypts a string using AES encryption
 */
export function encrypt(data: string): string {
  return CryptoJS.AES.encrypt(data, ENCRYPTION_KEY).toString();
}

/**
 * Decrypts an encrypted string
 */
export function decrypt(encryptedData: string): string {
  const bytes = CryptoJS.AES.decrypt(encryptedData, ENCRYPTION_KEY);
  return bytes.toString(CryptoJS.enc.Utf8);
}

/**
 * Sets a secure item in localStorage (encrypted)
 */
export function setSecureItem<T>(key: string, value: T): void {
  if (typeof window === "undefined") return;

  try {
    const jsonString = JSON.stringify(value);
    const encrypted = encrypt(jsonString);
    localStorage.setItem(key, encrypted);
  } catch (error) {
    console.error(`[secure-storage] Failed to save "${key}":`, error);
  }
}

/**
 * Gets a secure item from localStorage (decrypted)
 */
export function getSecureItem<T>(key: string): T | null {
  if (typeof window === "undefined") return null;

  try {
    const encrypted = localStorage.getItem(key);
    if (!encrypted) return null;

    const decrypted = decrypt(encrypted);
    return JSON.parse(decrypted) as T;
  } catch (error) {
    console.error(`[secure-storage] Failed to read "${key}":`, error);
    // If decryption fails, the data might be corrupted - remove it
    localStorage.removeItem(key);
    return null;
  }
}

/**
 * Removes a secure item from localStorage
 */
export function removeSecureItem(key: string): void {
  if (typeof window === "undefined") return;

  try {
    localStorage.removeItem(key);
  } catch (error) {
    console.error(`[secure-storage] Failed to remove "${key}":`, error);
  }
}

/**
 * Storage keys for auth data
 */
export const AUTH_STORAGE_KEYS = {
  USER: "auth_user",
  TOKEN: "auth_token",
} as const;
