import { Link, LinkProps } from "react-router-dom";
import { twMerge } from "tailwind-merge";
import getLinkVariant from "./getLinkVariant";

type CustomLinkProps = LinkProps & {
  variant?: "primary" | "secondary";
};

const CustomLink = ({ variant = "primary", className, ...rest }: CustomLinkProps) => {
  const v = getLinkVariant(variant);
  return <Link {...rest} className={twMerge(className, v)} />;
};

export default CustomLink;
