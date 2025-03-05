export type ErrorScreenProps = {
  message: string;
  status: number;
};

const ErrorScreen = ({ message, status }: ErrorScreenProps) => (
  <div className="py-8 px-4 mx-auto max-w-screen-xl lg:py-16 lg:px-6">
    <div className="mx-auto max-w-screen-sm text-center">
      <h1 className="mb-4 text-5xl tracking-tight  lg:text-9xl text-black">{status}</h1>
      <p className="mb-4 text-2xl font-light text-black">{message}</p>
    </div>
  </div>
);

export default ErrorScreen;
