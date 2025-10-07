"use client";
import React, {Suspense} from "react";
import AppointmentForm from "@/app/appointments/AppointmentForm";

export const dynamic = "force-dynamic";

export default function AppointmentsPage(){
    return (
        <Suspense fallback={<p className="p-8 text-gray-500">Loading Appointments...</p>}>
            <AppointmentForm />
        </Suspense>
    )
}