import useVariant from "@src/hooks/useVariant";
import { twMerge } from "tailwind-merge";

type LabelProps = React.LabelHTMLAttributes<HTMLLabelElement> & {
  variant?: "primary";
};

const Label = ({ variant = "primary", className, ...rest }: LabelProps) => {
  const v = useVariant(
    [
      {
        key: "primary",
        style: "",
      },
    ],
    variant,
    "text-gray-800 text-sm block"
  );

  return <label className={twMerge(className, v)} {...rest} />;
};

export default Label;
