import useVariant from "@src/hooks/useVariant";
import { twMerge } from "tailwind-merge";

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: "primary";
};

const Button = ({ variant = "primary", className, ...rest }: ButtonProps) => {
  const v = useVariant(
    [
      {
        key: "primary",
        style: "bg-blue-500 hover:bg-blue-600",
      },
    ],
    variant,
    "px-3 py-2 rounded text-white cursor-pointer"
  );

  return <button {...rest} className={twMerge(className, v)} />;
};

export default Button;
