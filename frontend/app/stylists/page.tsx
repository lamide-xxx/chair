"use client";
import React, { useEffect, useState } from "react";
import Link from "next/link";

interface Stylist {
  id: number;
  fullName: string;
  specialties: string[];
  rating: number;
  location: string;
  serviceIds: string[];
}

export default function StylistsPage() {
    const [stylists, setStylists] = useState<Stylist[]>([]);
    const [loading, setLoading] = useState(true);
    
    useEffect(() => {
        async function fetchStylists() {
            try {
                const response = await fetch("http://localhost:5170/api/stylists/");
                const data: Stylist[] = await response.json();
                setStylists(data);
            } catch (error) {
                console.error('Error fetching stylists:', error);
            } finally {
                setLoading(false);
            }
        }
        
        fetchStylists();
    }, []);
    
    if (loading) {
        return <p>Loading Stylists...</p>;
    }
    return(
        <div className={"p-8"}>
            <h1 className={"text-3xl font-bold mb-6"}>Stylists</h1>
            <ul className={"grid gap-6 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3"}>
                {stylists.map((stylist) => (
                    <li key={stylist.id}>
                        <h2 className={"text-xl font-semibold"}>{stylist.fullName}</h2>
                        <p className={"text-gray-600"}>Location: {stylist.location}</p>
                        <p className={"text-gray-600"}>Rating: {stylist.rating} / 5</p>
                        <p className={"text-gray-500"}>Specialties: {stylist.specialties.join(", ")}</p>
                        <Link
                            href={`stylists/${stylist.id}`}
                            className={"mt-3 inline-block text-blue-600 hover:underline"}
                        >
                            View Profile
                        </Link>
                    </li>
                ))}
            </ul>
        </div>
    )
}