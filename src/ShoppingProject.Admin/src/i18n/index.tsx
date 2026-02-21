"use client";

import { createContext, useContext, useState, useEffect, ReactNode } from "react";

import en from "./en.json";
import tr from "./tr.json";

export type Locale = "en" | "tr";

export const locales: Locale[] = ["en", "tr"];

const dictionaries: Record<Locale, typeof en> = {
  en,
  tr,
};

interface I18nContextType {
  locale: Locale;
  setLocale: (locale: Locale) => void;
  t: (key: string) => string;
}

const I18nContext = createContext<I18nContextType | undefined>(undefined);

const LANGUAGE_COOKIE_NAME = "locale";

export function I18nProvider({ children }: { children: ReactNode }) {
  const [locale, setLocaleState] = useState<Locale>("en");

  useEffect(() => {
    // Check for saved locale in localStorage or cookie
    const savedLocale = localStorage.getItem("locale") as Locale;
    if (savedLocale && locales.includes(savedLocale)) {
      setLocaleState(savedLocale);
    } else {
      // Check cookie
      const cookies = document.cookie.split(";");
      for (const cookie of cookies) {
        const [name, value] = cookie.trim().split("=");
        if (name === LANGUAGE_COOKIE_NAME && locales.includes(value as Locale)) {
          setLocaleState(value as Locale);
          break;
        }
      }
    }
  }, []);

  const setLocale = (newLocale: Locale) => {
    setLocaleState(newLocale);
    localStorage.setItem("locale", newLocale);
    // Set cookie for server-side
    document.cookie = `${LANGUAGE_COOKIE_NAME}=${newLocale};path=/;max-age=31536000;SameSite=Lax`;
  };

  const t = (key: string): string => {
    const keys = key.split(".");
    let value: any = dictionaries[locale];

    for (const k of keys) {
      if (value && typeof value === "object" && k in value) {
        value = value[k];
      } else {
        return key; // Return key if translation not found
      }
    }

    return typeof value === "string" ? value : key;
  };

  return <I18nContext.Provider value={{ locale, setLocale, t }}>{children}</I18nContext.Provider>;
}

export function useI18n() {
  const context = useContext(I18nContext);
  if (context === undefined) {
    throw new Error("useI18n must be used within an I18nProvider");
  }
  return context;
}
