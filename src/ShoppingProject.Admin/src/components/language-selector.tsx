"use client";

import { Globe } from "lucide-react";

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Button } from "@/components/ui/button";
import { useI18n, locales, Locale } from "@/i18n";

export function LanguageSelector() {
  const { locale, setLocale, t } = useI18n();

  const getLanguageLabel = (loc: Locale) => {
    return t(`common.${loc}`);
  };

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="ghost" size="icon" className="size-9">
          <Globe className="size-4" />
          <span className="sr-only">Toggle language</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        {locales.map((loc) => (
          <DropdownMenuItem key={loc} onClick={() => setLocale(loc)} className={locale === loc ? "bg-accent" : ""}>
            {getLanguageLabel(loc)}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
