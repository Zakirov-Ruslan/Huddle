import type { ComponentProps, CSSProperties } from "react";
import { Tooltip as ReactTooltip } from "react-tooltip";

const baseStyles: CSSProperties = {
    backgroundColor: "rgb(255, 255, 255)",
    color: "#222",
    borderRadius: "10px",
    fontWeight: "500",
    padding: "5px 10px 8px 10px",
    boxShadow: "0 20px 25px -5px rgb(0 0 0 / 0.1)",
};

type AppTooltipProps = ComponentProps<typeof ReactTooltip>;

/**
 * Single source of truth for the tooltip look, so we don't copy the same
 * inline style object into every component.
 */
export const AppTooltip = ({ style, ...props }: AppTooltipProps) => (
    <ReactTooltip
        {...props}
        opacity={1}
        border="1px solid #e8e8e8"
        style={{ ...baseStyles, ...style }}
    />
);
