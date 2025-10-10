"use client";
import { useState } from "react";

interface Stylist {
    id: number;
    fullName: string;
    specialties: string[];
    rating: number;
    location: string;
    serviceIds: string[];
    imageUrl?: string;
}

interface Recommendation {
    id: string;
    name: string;
    reason: string;
}

export default function FindMyStylistModal({stylists}: {stylists:Stylist[]}) {
    const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
    const [preference, setPreference] = useState<string>("");
    const [recommendations, setRecommendations] = useState<Recommendation[]>([]);
    const [loading, setLoading] = useState(false);
    
    async function handleFind(){
        try {
            setLoading(true);

            const res = await fetch(`${apiBaseUrl}/api/ai/recommend-stylist`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ preference, stylists }),
            });

            const data = await res.json();

            // Extract the AI's message content
            const content = data?.choices?.[0]?.message?.content;
            if (!content) {
                console.error("No content returned from AI");
                setRecommendations([]);
                return;
            }

            // Clean out the Markdown formatting (```json ... ```)
            const cleaned = content.replace(/```json|```/g, "").trim();

            // Parse into actual JS array
            const recommendations = JSON.parse(cleaned);

            // Save into state
            setRecommendations(recommendations);
        } catch (err) {
            console.error("Error fetching or parsing recommendations:", err);
        } finally {
            setLoading(false);
        }
    }
    
    return(
        <div className="p-4 bg-white rounded-lg shadow-md">
            <h2 className="text-lg font-bold mb-2"> Find My Stylist</h2>
            <input
                value={preference}
                onChange={(e) => setPreference(e.target.value)}
                placeholder="e.g. braids near London"
                className="border p-2 rounded w-full mb-4"
            />
            <button
                onClick={handleFind}
                className="bg-black text-white px-4 py-2 rounded"
            >
                {loading ? "Thinking..." : "Find My Stylist"}
            </button>

            {
                recommendations.length > 0 && (
                    <div className="mt-4 space-y-2">
                        {
                            recommendations.map((r) => (
                                <div key={r.id} className="border p-3 rounded">
                                    <p className="font-semibold">{r.name}</p>
                                    <p className="text-sm text-gray-600">{r.reason}</p>
                                </div>
                            ))
                        }
                    </div>
                )
            }
            
        </div>
    )
    
    
}