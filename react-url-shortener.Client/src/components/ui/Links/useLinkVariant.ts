import useVariant from "@src/hooks/useVariant";
import "./Links.css"

const useLinkVariant = (variant: string) => {
  const v = useVariant(
    [
      {
        key: "primary",
        style: "link-primary",
      },
      {
        key: "secondary",
        style: "link-secondary",
      },
    ],
    variant,
    "link-default"
  );

  return v;
};

export default useLinkVariant;
