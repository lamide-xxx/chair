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

interface Service {
    id: number;
    name: string;
    price: number;
    durationMinutes: number;
}

export default function StylistProfilePage(){
    const params = useParams();
    const router = useRouter();
    
    const [stylist, setStylist] = useState<Stylist | null>(null);
    const [services, setServices] = useState<Service[]>([]);
    const [loading, setLoading] = useState(true);
    useEffect(() => {
        async function fetchStylistProfile(){
            try{
                // fetch stylist details
                const stylistResponse = await fetch(`http://localhost:5170/api/stylists/${params.id}`);
                if (!stylistResponse.ok) {
                    throw new Error(`HTTP error fetching stylist! status: ${stylistResponse.status}; message: ${stylistResponse.statusText}, body: ${await stylistResponse.text()}`);
                }
                const stylistData: Stylist = await stylistResponse.json();
                setStylist(stylistData);

                // fetch all services
                const servicesResponse = await fetch("http://localhost:5170/api/services");
                const servicesData: Service[] = await servicesResponse.json();
                
                // filter services to only those offered by this stylist
                const filteredServices = servicesData.filter(service => stylistData.serviceIds.includes(service.id.toString()));
                setServices(filteredServices);

            }catch (err){
                console.error('Error fetching stylist profile:', err);
                router.push("/stylists");
            }finally{
                setLoading(false);
            }
        }
        
        fetchStylistProfile();
    }, [])
    
    if (loading){
        return <p className={"p-8 text-gray-500"}>Loading Stylist...</p>;
    }
    if (!stylist){
        return <p className={"p-8 text-red-500"}>Stylist not found.</p>;
    }
    return(
        <div className="bg-gray-50 min-h-screen py-10">
            <div className="max-w-4xl mx-auto px-6">
                {/* Stylist Info Card */}
                <div className="bg-white rounded-2xl shadow p-8 mb-10 text-center">
                    {/* Placeholder for photo/avatar */}
                    <div className="w-24 h-24 mx-auto mb-4 rounded-full bg-gradient-to-tr from-blue-300 to-blue-600 flex items-center justify-center text-white text-2xl font-bold">
                        {stylist.fullName.charAt(0)}
                    </div>
                    <h1 className="text-3xl font-extrabold text-gray-800">
                        {stylist.fullName}
                    </h1>
                    <p className="text-gray-600 mt-2">📍 {stylist.location}</p>
                    <p className="text-yellow-500 font-semibold mt-1">
                        ⭐ {stylist.rating} / 5
                    </p>
                    <p className="mt-3 text-gray-600">
                        Specialties: {stylist.specialties.join(", ")}
                    </p>
                </div>

                {/* Services */}
                <h2 className="text-2xl font-bold mb-6 text-gray-800">Services</h2>
                {services.length > 0 ? (
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
                        {services.map((service) => (
                            <div
                                key={service.id}
                                className="border rounded-xl p-5 shadow-sm bg-white hover:shadow-md transition"
                            >
                                <h3 className="font-semibold text-lg mb-1">{service.name}</h3>
                                <p className="text-sm text-gray-600 mb-2">
                                    {service.durationMinutes} mins
                                </p>
                                <p className="text-blue-600 font-semibold mb-4">
                                    £{service.price}
                                </p>
                                <button
                                    className="w-full bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600 transition"
                                    onClick={() =>
                                        router.push(
                                            `/appointments?stylistId=${params.id}&serviceId=${service.id}`
                                        )
                                    }
                                >
                                    Book Appointment
                                </button>
                            </div>
                        ))}
                    </div>
                ) : (
                    <p className="text-gray-500">
                        This stylist hasn&apos;t listed any services yet.
                    </p>
                )}
            </div>
        </div>
    );
}