import React, { useEffect, useState } from 'react';
import { Search, MapPin, Star, Filter } from 'lucide-react';
import Navbar from '../components/layout/Navbar';
import Footer from '../components/layout/Footer';
import { getWorkshops, getWorkshopDocuments } from '../api/workshops';

const WorkshopCard = ({ workshop }) => {
  const [image, setImage] = useState(null);

  useEffect(() => {
    const fetchImage = async () => {
      try {
        const docs = await getWorkshopDocuments(workshop.id, { pageNumber: 1, pageSize: 50 });
        // Priority: Logo (1) -> Gallery (2)
        const logo = docs.items.find(d => d.type === 1);
        const gallery = docs.items.find(d => d.type === 2);
        if (logo) setImage(logo.publicUrl);
        else if (gallery) setImage(gallery.publicUrl);
      } catch (e) {
        console.error("Failed to fetch documents for workshop", workshop.id, e);
      }
    };
    fetchImage();
  }, [workshop.id]);

  return (
    <div className="bg-white dark:bg-dark-card rounded-2xl border border-slate-100 dark:border-slate-700 overflow-hidden hover:shadow-lg transition-all group">
      <div className="h-48 bg-slate-200 dark:bg-slate-700 relative">
        {image ? (
          <img 
            src={image} 
            alt={workshop.displayName} 
            className="w-full h-full object-cover"
          />
        ) : (
          <div className="absolute inset-0 flex items-center justify-center text-slate-400">
            No Image
          </div>
        )}
      </div>
      <div className="p-6">
        <div className="flex justify-between items-start mb-2">
          <h3 className="text-xl font-bold text-slate-900 dark:text-white group-hover:text-primary transition-colors">
            {workshop.displayName}
          </h3>
          <div className="flex items-center gap-1 bg-yellow-100 dark:bg-yellow-900/30 px-2 py-1 rounded-lg">
            <Star className="w-4 h-4 text-yellow-500 fill-yellow-500" />
            <span className="text-sm font-bold text-yellow-700 dark:text-yellow-400">4.8</span>
          </div>
        </div>
        <p className="text-slate-500 dark:text-slate-400 text-sm mb-4 flex items-center gap-1">
          <MapPin className="w-4 h-4" /> {workshop.address}, {workshop.city}
        </p>
        <div className="flex flex-wrap gap-2 mb-6">
          <span className="px-3 py-1 bg-slate-100 dark:bg-slate-800 text-slate-600 dark:text-slate-300 text-xs font-medium rounded-full">
            Oil Change
          </span>
          <span className="px-3 py-1 bg-slate-100 dark:bg-slate-800 text-slate-600 dark:text-slate-300 text-xs font-medium rounded-full">
            Brakes
          </span>
        </div>
        <button className="w-full py-2.5 border border-primary text-primary hover:bg-primary hover:text-white font-medium rounded-xl transition-all">
          View Details
        </button>
      </div>
    </div>
  );
};

const FindWorkshopPage = () => {
  const [workshops, setWorkshops] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchWorkshops = async () => {
      try {
        const data = await getWorkshops({ pageNumber: 1, pageSize: 10 });
        setWorkshops(data.items || []);
      } catch (e) {
        console.error("Failed to fetch workshops", e);
      } finally {
        setLoading(false);
      }
    };
    fetchWorkshops();
  }, []);

  return (
    <div className="min-h-screen bg-gradient-to-br from-secondary/10 via-slate-50 to-primary/10 dark:bg-slate-900 dark:bg-none transition-colors">
      <Navbar />
      <div className="pt-32 pb-12 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-slate-900 dark:text-white mb-4">Find a Workshop</h1>
          <div className="flex flex-col md:flex-row gap-4">
            <div className="flex-1 relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
              <input
                type="text"
                placeholder="Search by service (e.g., Oil Change, Brakes)"
                className="w-full pl-10 pr-4 py-3 bg-white dark:bg-dark-card border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent outline-none transition-all dark:text-white"
              />
            </div>
            <div className="flex-1 relative">
              <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
              <input
                type="text"
                placeholder="Location (City or Zip Code)"
                className="w-full pl-10 pr-4 py-3 bg-white dark:bg-dark-card border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent outline-none transition-all dark:text-white"
              />
            </div>
            <button className="px-6 py-3 bg-primary hover:bg-primary-hover text-white font-medium rounded-xl shadow-lg shadow-primary/25 transition-all">
              Search
            </button>
          </div>
        </div>

        <div className="grid lg:grid-cols-4 gap-8">
          {/* Filters Sidebar */}
          <div className="hidden lg:block space-y-6">
            <div className="bg-white dark:bg-dark-card p-6 rounded-2xl border border-slate-100 dark:border-slate-700">
              <div className="flex items-center gap-2 mb-4">
                <Filter className="w-5 h-5 text-slate-500" />
                <h3 className="font-bold text-slate-900 dark:text-white">Filters</h3>
              </div>
              {/* Add filter options here */}
              <div className="space-y-3">
                <label className="flex items-center gap-2 text-slate-600 dark:text-slate-300">
                  <input type="checkbox" className="rounded border-slate-300 text-primary focus:ring-primary" />
                  Open Now
                </label>
                <label className="flex items-center gap-2 text-slate-600 dark:text-slate-300">
                  <input type="checkbox" className="rounded border-slate-300 text-primary focus:ring-primary" />
                  Verified Only
                </label>
              </div>
            </div>
          </div>

          {/* Results Grid */}
          <div className="lg:col-span-3 grid md:grid-cols-2 gap-6">
            {loading ? (
              <div className="col-span-2 text-center py-12 text-slate-500">Loading workshops...</div>
            ) : workshops.length > 0 ? (
              workshops.map((workshop) => (
                <WorkshopCard key={workshop.id} workshop={workshop} />
              ))
            ) : (
              <div className="col-span-2 text-center py-12 text-slate-500">No workshops found.</div>
            )}
          </div>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default FindWorkshopPage;
