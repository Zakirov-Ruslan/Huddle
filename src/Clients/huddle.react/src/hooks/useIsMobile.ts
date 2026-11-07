import { useMediaQuery } from "usehooks-ts";

/**
 * Tailwind's `md` breakpoint is 48rem (768px).
 * Keep these queries in sync with the `md:` / `max-md:` variants used in markup.
 */
export const MOBILE_MEDIA_QUERY = "(max-width: 767px)";

/**
 * `true` on phones, where the channel list and the chat are two separate screens.
 *
 * Prefer pure CSS (`hidden` / `max-md:hidden`) for layout switching,
 * this hook is only needed where a *behaviour* depends on the viewport
 * (e.g. skipping the "open the first channel" redirect on phones).
 */
export const useIsMobile = () =>
    useMediaQuery(MOBILE_MEDIA_QUERY, { initializeWithValue: true });
