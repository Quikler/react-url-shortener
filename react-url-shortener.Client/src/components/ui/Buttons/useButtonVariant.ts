import useVariant from "@src/hooks/useVariant";

const useButtonVariant = (variant: string) => {
  const v = useVariant(
    [
      {
        key: "primary",
        style: "btn-primary",
      },
      {
        key: "secondary",
        style: "btn-secondary",
      },
      {
        key: "danger",
        style: "btn-danger",
      },
      {
        key: "info",
        style: "btn-info",
      },
    ],
    variant,
    "btn-default"
  );

  return v;
};

export default useButtonVariant;