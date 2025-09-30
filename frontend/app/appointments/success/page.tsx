import Link from "next/link";

export default function AppointmentSuccessPage(){
    return(
        <div className={"flex items-center justify-center min-h-screen bg-gradient-to-br from-blue-300 via-blue-500 to-blue-700"}>
            <div className={"bg-white rounded-xl shadow-lg p-10 max-w-md text-center"}>
                <h1 className={"text-4xl font-bold text-blue-600 mb-4"}>🎉 Success!</h1>
                <p className={"text-gray-700 mb-8"}>Your appointment has been booked successfully.</p>
                <div className="space-x-4">
                    <Link
                        href="/stylists"
                        className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition"
                    >
                        Back to Stylists
                    </Link>
                    <Link
                        href="/appointments/my"
                        className="bg-gray-200 text-gray-700 px-6 py-2 rounded-lg hover:bg-gray-300 transition"
                    >
                        View Appointments
                    </Link>
                </div>
            </div>
        </div>
    )
}