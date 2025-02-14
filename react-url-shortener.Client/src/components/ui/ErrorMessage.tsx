import React from "react";

type ErrorMessageProps = React.HTMLAttributes<HTMLParagraphElement> & {};

const ErrorMessage = ({ ...rest }: ErrorMessageProps) => <p className="text-red-500" {...rest} />;

export default ErrorMessage;
