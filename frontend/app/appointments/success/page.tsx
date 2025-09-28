import Link from "next/link";

export default function AppointmentSuccessPage(){
    return(
        <div className={"p-8 text-center"}>
            <h1 className={"text-3xl font-bold mb-4"}>🎉 Appointment Booked!</h1>
            <p className={"mb-6"}>Your appointment has successfully been created.</p>
            <Link 
                href={"/stylists"}
                className={"text-blue-600 hover:underline font-semibold"}
            >
                Back to Stylists
            </Link>
        </div>
    )
}