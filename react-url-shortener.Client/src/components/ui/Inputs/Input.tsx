import React from "react";
import { twMerge } from "tailwind-merge";
import getInputVariant from "./getInputVariant";

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  variant?: "primary" | "secondary";
}

const Input = React.forwardRef<HTMLInputElement, InputProps>(
  ({ variant = "primary", className, ...rest }, ref) => {
    const v = getInputVariant(variant);
    return <input {...rest} ref={ref} className={twMerge(className, v)} />;
  }
);

export default Input;
