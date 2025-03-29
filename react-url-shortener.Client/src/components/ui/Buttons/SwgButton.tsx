import { getVariant } from "@src/utils/helpers";
import React from "react";
import { twMerge } from "tailwind-merge";

type SwgButtonProps = React.HTMLAttributes<HTMLButtonElement> & {
  variant?: "primary";
};

const SwgButton = ({ variant = "primary", className, ...rest }: SwgButtonProps) => {
  const v = getVariant([
    {
      "key": "primary",
      "style": "focus:outline-blue-500"
    },
  ], variant, "rounded focus:outline-1 focus:outline outline-offset-4");

  return <button className={twMerge(v, className)} {...rest} />
}

export default SwgButton;
