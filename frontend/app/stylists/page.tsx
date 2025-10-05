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
  imageUrl?: string;
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
        <div className="p-8">
            <h1 className="text-3xl font-bold mb-6">Stylists</h1>
            <ul className="grid gap-6 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3">
                {stylists.map((stylist, idx) => (
                    <li
                        key={stylist.id}
                        className="bg-white rounded-lg shadow overflow-hidden flex flex-col hover:shadow-lg duration-300 hover:scale-102 cursor-pointer"
                    >
                        <Link
                            href={`/stylists/${stylist.id}`}
                            className="flex flex-col h-full"
                        >
                            {/* Image */}
                            <div className="h-48 w-full bg-gray-200">
                                <img
                                    src={
                                        stylist.imageUrl ||
                                        `https://picsum.photos/400/300?random=${idx + 1}`
                                    }
                                    alt={stylist.fullName}
                                    className="h-full w-full object-cover"
                                />
                            </div>
    
                            {/* Info */}
                            <div className="p-6 flex flex-col justify-between flex-grow">
                                <div>
                                    <h2 className="text-xl font-semibold mb-2">
                                        {stylist.fullName}
                                    </h2>
                                    <p className="text-gray-600 mb-1">📍 {stylist.location}</p>
                                    <p className="text-gray-600 mb-1">⭐ {stylist.rating} / 5</p>
                                    <p className="text-gray-500">
                                        {stylist.specialties.length > 0 ? `Specialties: ${stylist.specialties.join(", ")}` : "No specialties listed"}
                                    </p>
                                </div>
    
                                {/* Book Button */}
                                <div className="mt-4">
                                    <span
                                        onClick={(e) => {
                                            e.preventDefault(); // stop Link navigation
                                            window.location.href = `/appointments?stylistId=${stylist.id}`;
                                        }}
                                        className="inline-block bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 transition cursor-pointer"
                                    >
                                        Book
                                    </span>
                                </div>
                            </div>
                        </Link>
                    </li>
                ))}
            </ul>
        </div>
    )
}