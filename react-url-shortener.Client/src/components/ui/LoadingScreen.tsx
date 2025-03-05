const LoadingScreen = () => (
  <div className="flex flex-col items-center p-6 my-24 rounded-lg border-gray-300">
    <div className="transform">
    <div className="p-4 bg-gradient-to-tr animate-spin from-green-500 to-blue-500 via-sky-500 rounded-full">
      <div className="bg-white rounded-full">
        <div className="w-24 h-24 rounded-full" />
      </div>
    </div>
  </div>
  </div>
);

export default LoadingScreen;
