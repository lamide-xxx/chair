"use client";
import { useEffect, useState } from "react";

interface Service {
    id: number;
    name: string;
    price: number;
    durationMinutes: number;
}
export default function ServicesPage(){
    const [services, setServices] = useState<Service[]>([]);
    const [loading, setLoading] = useState(true);
    
    useEffect(() => {
        async function fetchServices(){
            try {
                const response = await fetch("http://localhost:5170/api/services/");
                const data: Service[] = await response.json();
                setServices(data);
            } catch (error) {
                console.error('Error fetching services:', error);
            } finally {
                setLoading(false);
            }
        }
        fetchServices();
    }, []);
    
    if(loading){
        return <p>Loading Services...</p>;
    };
    
    return (
        <div className={"p-8"}>
            <h1 className={"text-3xl font-bold mb-6"}>Services</h1>
            <ul className={"space-y-4"}>
                {services.map((service) => (
                    <li key={service.id}>
                        <h2 className={"text-xl font-semibold"}>{service.name}</h2>
                        <p>Price: £{service.price}</p>
                        <p>Duration: {service.durationMinutes} mins</p>
                    </li>
                ))}
            </ul>
        </div>
    );
}