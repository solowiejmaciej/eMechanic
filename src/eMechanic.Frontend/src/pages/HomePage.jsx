import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Car, Plus, Calendar, Wrench, Search, Filter } from 'lucide-react';
import Navbar from '../components/layout/Navbar';
import Footer from '../components/layout/Footer';
import { useAuth } from '../context/AuthContext';
import api from '../api/client';

const HomePage = () => {
  const { user } = useAuth();
  const [vehicles, setVehicles] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchVehicles = async () => {
      try {
        // Fetch vehicles for the logged-in user
        // Using the endpoint from OpenAPI spec: GET /api/v1/vehicles
        const response = await api.get('/api/v1/vehicles', {
          params: {
            PageNumber: 1,
            PageSize: 10
          }
        });
        setVehicles(response.data.items || []);
      } catch (error) {
        console.error("Failed to fetch vehicles:", error);
      } finally {
        setLoading(false);
      }
    };

    if (user) {
      fetchVehicles();
    }
  }, [user]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-secondary/10 via-slate-50 to-primary/10 dark:bg-slate-900 dark:bg-none transition-colors flex flex-col">
      <Navbar />
      
      <main className="flex-grow pt-24 pb-12 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full">
        {/* Header */}
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8">
          <div>
            <h1 className="text-3xl font-bold text-slate-900 dark:text-white">
              My Garage
            </h1>
            <p className="text-slate-600 dark:text-slate-400 mt-1">
              Manage your vehicles and repair history
            </p>
          </div>
          <button className="flex items-center justify-center gap-2 px-4 py-2 bg-primary hover:bg-primary-hover text-white rounded-xl transition-colors shadow-lg shadow-primary/25">
            <Plus className="w-5 h-5" />
            <span>Add Vehicle</span>
          </button>
        </div>

        {/* Stats / Quick Actions */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          <div className="bg-white dark:bg-dark-card p-6 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800">
            <div className="flex items-center gap-4">
              <div className="p-3 bg-blue-50 dark:bg-blue-900/20 rounded-xl text-blue-600 dark:text-blue-400">
                <Car className="w-6 h-6" />
              </div>
              <div>
                <p className="text-sm text-slate-500 dark:text-slate-400">Total Vehicles</p>
                <p className="text-2xl font-bold text-slate-900 dark:text-white">{vehicles.length}</p>
              </div>
            </div>
          </div>
          
          <div className="bg-white dark:bg-dark-card p-6 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800">
            <div className="flex items-center gap-4">
              <div className="p-3 bg-orange-50 dark:bg-orange-900/20 rounded-xl text-orange-600 dark:text-orange-400">
                <Wrench className="w-6 h-6" />
              </div>
              <div>
                <p className="text-sm text-slate-500 dark:text-slate-400">Active Repairs</p>
                <p className="text-2xl font-bold text-slate-900 dark:text-white">0</p>
              </div>
            </div>
          </div>

          <div className="bg-white dark:bg-dark-card p-6 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800">
            <div className="flex items-center gap-4">
              <div className="p-3 bg-green-50 dark:bg-green-900/20 rounded-xl text-green-600 dark:text-green-400">
                <Calendar className="w-6 h-6" />
              </div>
              <div>
                <p className="text-sm text-slate-500 dark:text-slate-400">Upcoming Visits</p>
                <p className="text-2xl font-bold text-slate-900 dark:text-white">0</p>
              </div>
            </div>
          </div>
        </div>

        {/* Vehicles List */}
        <div className="bg-white dark:bg-dark-card rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800 overflow-hidden">
          <div className="p-6 border-b border-slate-100 dark:border-slate-800 flex flex-col sm:flex-row sm:items-center justify-between gap-4">
            <h2 className="text-lg font-bold text-slate-900 dark:text-white">Vehicles</h2>
            <div className="flex items-center gap-2">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" />
                <input 
                  type="text" 
                  placeholder="Search vehicles..." 
                  className="pl-9 pr-4 py-2 bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-lg text-sm focus:ring-2 focus:ring-primary focus:border-transparent outline-none dark:text-white w-full sm:w-64"
                />
              </div>
              <button className="p-2 text-slate-600 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-800 rounded-lg border border-slate-200 dark:border-slate-700">
                <Filter className="w-4 h-4" />
              </button>
            </div>
          </div>

          {loading ? (
            <div className="p-12 text-center">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary mx-auto mb-4"></div>
              <p className="text-slate-500 dark:text-slate-400">Loading your garage...</p>
            </div>
          ) : vehicles.length > 0 ? (
            <div className="divide-y divide-slate-100 dark:divide-slate-800">
              {vehicles.map((vehicle) => (
                <div key={vehicle.id} className="p-6 hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors flex items-center justify-between group">
                  <div className="flex items-center gap-4">
                    <div className="w-12 h-12 bg-slate-100 dark:bg-slate-800 rounded-xl flex items-center justify-center text-slate-500 dark:text-slate-400">
                      <Car className="w-6 h-6" />
                    </div>
                    <div>
                      <h3 className="font-semibold text-slate-900 dark:text-white">
                        {vehicle.manufacturer} {vehicle.model}
                      </h3>
                      <p className="text-sm text-slate-500 dark:text-slate-400">
                        {vehicle.licensePlate} • {vehicle.productionYear}
                      </p>
                    </div>
                  </div>
                  <div className="flex items-center gap-4">
                    <span className="px-3 py-1 bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-400 text-xs font-medium rounded-full">
                      Active
                    </span>
                    <button className="opacity-0 group-hover:opacity-100 p-2 text-slate-400 hover:text-primary transition-all">
                      <Wrench className="w-4 h-4" />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="p-12 text-center">
              <div className="w-16 h-16 bg-slate-100 dark:bg-slate-800 rounded-full flex items-center justify-center mx-auto mb-4 text-slate-400">
                <Car className="w-8 h-8" />
              </div>
              <h3 className="text-lg font-medium text-slate-900 dark:text-white mb-2">No vehicles found</h3>
              <p className="text-slate-500 dark:text-slate-400 mb-6 max-w-sm mx-auto">
                Add your first vehicle to start tracking repairs and maintenance history.
              </p>
              <button className="px-6 py-2.5 bg-primary hover:bg-primary-hover text-white rounded-xl transition-colors shadow-lg shadow-primary/25 font-medium">
                Add Your First Car
              </button>
            </div>
          )}
        </div>
      </main>
      
      <Footer />
    </div>
  );
};

export default HomePage;
