"use client";
import { useEffect, useState } from "react";
import {useParams, useRouter} from "next/navigation";

interface Stylist {
    id: number;
    fullName: string;
    specialties: string[];
    rating: number;
    location: string;
    serviceIds: string[];
}

export default function StylistProfilePage(){
    const params = useParams();
    const router = useRouter();
    const [stylist, setStylist] = useState<Stylist | null>(null);
    const [loading, setLoading] = useState(true);
    useEffect(() => {
        async function fetchStylist(){
            try{
                const response = await fetch(`http://localhost:5170/api/stylists/${params.id}`);
                if (!response.ok) {
                    throw new Error(`HTTP error fetching stylist! status: ${response.status}; message: ${response.statusText}, body: ${await response.text()}`);
                }
                const stylistData: Stylist = await response.json();
                setStylist(stylistData);
            }catch (err){
                console.error('Error fetching stylist:', err);
                router.push("/stylists");
            }finally{
                setLoading(false);
            }
        }
        fetchStylist();
    }, [])
    
    if (loading){
        return <p>Loading Stylist...</p>;
    }
    if (!stylist){
        return <p>Stylist not found.</p>;
    }
    return(
        <div className={"p-8 max-w-xl mx-auto"}>
            <h1 className={"text-3xl font-bold mb-4"}>{stylist.fullName}</h1>
            <p className={"mb-2 text-gray-600"}>Location: {stylist.location}</p>
            <p className={"mb-2 text-gray-600"}>Rating: {stylist.rating} / 5</p>
            <p className={"mb-2 text-gray-600"}>Specialties: {stylist.specialties.join(", ")}</p>
            <p className={"mb-2 text-gray-600"}>Services Offered (IDs): {stylist.serviceIds.join(", ")}</p>

            {/* Future: Embed Instagram or photos here */}
            
            <button 
                className={"bg-blue-500 text-white px-4 py-2 rounded"}
                onClick={() => router.push(`/appointments?stylistId=${params.id}`)}
            >
                Book Appointment
            </button>
        </div>
    );
}