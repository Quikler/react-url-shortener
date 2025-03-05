import useVariant from "@src/hooks/useVariant";
import { twMerge } from "tailwind-merge";

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: "primary" | "secondary" | "danger" | "info";
};

const Button = ({ variant = "primary", className, ...rest }: ButtonProps) => {
  const v = useVariant(
    [
      {
        key: "primary",
        style: "bg-blue-500 hover:bg-blue-600",
      },
      {
        key: "secondary",
        style: "bg-sky-500 hover:bg-sky-600",
      },
      {
        key: "danger",
        style: "bg-red-500 hover:bg-red-600",
      },
      {
        key: "info",
        style: "bg-indigo-500 hover:bg-indigo-600",
      },
    ],
    variant,
    "px-3 py-2 rounded text-white cursor-pointer disabled:opacity-65"
  );

  return <button {...rest} className={twMerge(className, v)} />;
};

export default Button;
