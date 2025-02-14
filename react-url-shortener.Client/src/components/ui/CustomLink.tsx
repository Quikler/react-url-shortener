import useVariant from "@src/hooks/useVariant";
import { Link, LinkProps } from "react-router-dom";
import { twMerge } from "tailwind-merge";

type CustomLinkProps = LinkProps & {
  variant?: "primary" | "secondary";
};

const CustomLink = ({ variant = "primary", className, ...rest }: CustomLinkProps) => {
  const v = useVariant(
    [
      {
        key: "primary",
        style: "text-blue-500 hover:text-blue-600",
      },
      {
        key: "secondary",
        style: "text-blue-300 hover:text-blue-400",
      },
    ],
    variant,
    "font-bold"
  );

  return <Link {...rest} className={twMerge(className, v)} />;
};

export default CustomLink;
