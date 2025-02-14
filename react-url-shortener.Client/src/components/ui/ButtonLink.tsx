import useVariant from "@src/hooks/useVariant";
import { Link, LinkProps } from "react-router-dom";
import { twMerge } from "tailwind-merge";

type ButtonLinkProps = LinkProps & {
  variant?: "primary" | "secondary";
};

const ButtonLink = ({ variant = "primary", className, ...rest }: ButtonLinkProps) => {
  const v = useVariant(
    [
      {
        key: "primary",
        style: "bg-blue-500 hover:bg-blue-600",
      },
      {
        key: "secondary",
        style: "bg-sky-500 hover:bg-sky-600",
      }
    ],
    variant,
    "px-3 py-2 rounded text-white cursor-pointer"
  );

  return <Link {...rest} className={twMerge(className, v)} />;
};

export default ButtonLink;
