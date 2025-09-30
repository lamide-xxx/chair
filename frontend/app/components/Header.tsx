import Link from "next/link";

export default function Header() {
    return(
        <header className="bg-gray-100 shadow p-4 flex justify-between items-center">
            <h1 className="text-xl font-bold">
                <Link href={"/"}>Chair</Link>
            </h1>
            <nav className="space-x-4">
                <Link href={"/appointments"} className={"hover:underline"}>Book</Link>
                <Link href={"/appointments/my"} className={"hover:underline"}>My Appointments</Link>
                <Link href={"/stylists"} className={"hover:underline"}>Stylists</Link>
            </nav>
        </header>
    )
}