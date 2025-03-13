import React from "react";
import { twMerge } from "tailwind-merge";
import useInputVariant from "./useInputVariant";

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  variant?: "primary" | "secondary";
}

const Input = React.forwardRef<HTMLInputElement, InputProps>(
  ({ variant = "primary", className, ...rest }, ref) => {
    const v = useInputVariant(variant);
    return <input {...rest} ref={ref} className={twMerge(v)} />;
  }
);

export default Input;
