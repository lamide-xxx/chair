"use client";
import Link from "next/link";
import {usePathname} from "next/navigation";

export default function Header() {
    const pathname = usePathname();
    const linkClass = (path: string) =>
        `hover:underline ${pathname === path ? "font-bold text-blue-600" : ""}`;
    
    return(
        <header className="bg-gray-100 shadow p-4 flex justify-between items-center">
            <h1 className="text-xl font-bold">
                <Link href={"/"}>Chair</Link>
            </h1>
            <nav className="space-x-4">
                <Link href={"/appointments"} className={linkClass("/appointments")}>Book</Link>
                <Link href={"/appointments/my"} className={linkClass("/appointments/my")}>My Appointments</Link>
                <Link href={"/stylists"} className={linkClass("/stylists")}>Stylists</Link>
            </nav>
        </header>
    )
}