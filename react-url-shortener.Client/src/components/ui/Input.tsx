import React from "react";
import { twMerge } from "tailwind-merge";

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  variant?: "primary" | "secondary";
}

const useVariant = (
  variants: { key: string; style: string }[],
  variant: string,
  defaultStyle: string
) => {
  const foundVariant = variants.find((v) => v.key === variant);
  return [defaultStyle, foundVariant?.style].filter(Boolean).join(" ").trim();
};

const Input = React.forwardRef<HTMLInputElement, InputProps>(
  ({ variant = "primary", className, ...rest }, ref) => {
    const v = useVariant(
      [
        {
          key: "primary",
          style: "outline-blue-600 border-gray-300",
        },
        {
          key: "secondary",
          style: "outline-gray-600 border-gray-500",
        },
      ],
      variant,
      "text-gray-800 border px-4 py-3 rounded-md"
    );

    return <input {...rest} ref={ref} className={twMerge(className, v)} />;
  }
);

export default Input;
