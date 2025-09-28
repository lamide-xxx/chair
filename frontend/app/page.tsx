import Link from "next/link";

export default function HomePage() {
  return (
    <div className={"flex flex-col items-center justify-center py-20 bg-gradient-to-br from-blue-300 via-blue-500 to-blue-700 text-white "}>
        <h1 className={"text-5xl font-extrabold mb-4"}>Chair</h1>
        <p className={"text-xl mb-8"}>Booking Stylists With Ease</p>
        <div className={"flex space-x-4"}>
            <Link
                href={"/stylists"}
                className={"bg-white text-blue-600 font-semibold px-6 py-3 rounded-lg shadow hover:bg-gray-100 transition"}
            >
                Find a Stylist
            </Link>
            <Link
                href="/appointments"
                className="border border-white text-white font-semibold px-6 py-3 rounded-lg hover:bg-white hover:text-blue-600 transition"
            >
                Book Appointment
            </Link>
            
        </div>
    </div>
  );
}
