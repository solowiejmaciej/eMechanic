import React from 'react';
import { motion } from 'framer-motion';
import { Wrench, Car, Clock, ShieldCheck, DollarSign, BarChart3, Search } from 'lucide-react';

const FeatureCard = ({ icon: Icon, title, description, delay }) => (
  <motion.div
    initial={{ opacity: 0, y: 20 }}
    whileInView={{ opacity: 1, y: 0 }}
    viewport={{ once: true }}
    transition={{ duration: 0.5, delay }}
    className="p-6 rounded-2xl bg-white dark:bg-dark-card border border-slate-100 dark:border-slate-700 shadow-sm hover:shadow-xl transition-all duration-300 group"
  >
    <div className="w-12 h-12 bg-slate-50 dark:bg-slate-800 rounded-xl flex items-center justify-center mb-4 group-hover:bg-primary/10 transition-colors">
      <Icon className="w-6 h-6 text-slate-600 dark:text-slate-400 group-hover:text-primary transition-colors" />
    </div>
    <h3 className="text-xl font-bold text-slate-900 dark:text-white mb-2">{title}</h3>
    <p className="text-slate-600 dark:text-slate-400 leading-relaxed">{description}</p>
  </motion.div>
);

const FeaturesSection = () => {
  return (
    <section className="py-24 bg-transparent transition-colors">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="text-center max-w-3xl mx-auto mb-16">
          <h2 className="text-3xl lg:text-4xl font-bold text-slate-900 dark:text-white mb-4">
            Everything you need to <br />
            <span className="text-primary">manage vehicle repairs</span>
          </h2>
          <p className="text-lg text-slate-600 dark:text-slate-400">
            Whether you own a car or run a workshop, eMechanic streamlines the entire process.
          </p>
        </div>

        <div className="grid md:grid-cols-2 gap-12">
          {/* For Owners */}
          <div>
            <div className="flex items-center gap-3 mb-8">
              <div className="p-2 bg-blue-100 dark:bg-blue-900/30 rounded-lg">
                <Car className="w-6 h-6 text-primary" />
              </div>
              <h3 className="text-2xl font-bold text-slate-900 dark:text-white">For Car Owners</h3>
            </div>
            <div className="grid gap-6">
              <FeatureCard
                icon={Search}
                title="Find Trusted Mechanics"
                description="Browse verified workshops with real reviews and ratings from other car owners."
                delay={0.1}
              />
              <FeatureCard
                icon={DollarSign}
                title="Transparent Pricing"
                description="Get detailed quotes upfront. No hidden fees or surprise charges."
                delay={0.2}
              />
              <FeatureCard
                icon={Clock}
                title="Real-time Updates"
                description="Track your repair status live and get notified when your car is ready."
                delay={0.3}
              />
            </div>
          </div>

          {/* For Workshops */}
          <div>
            <div className="flex items-center gap-3 mb-8">
              <div className="p-2 bg-orange-100 dark:bg-orange-900/30 rounded-lg">
                <Wrench className="w-6 h-6 text-secondary" />
              </div>
              <h3 className="text-2xl font-bold text-slate-900 dark:text-white">For Workshops</h3>
            </div>
            <div className="grid gap-6">
              <FeatureCard
                icon={BarChart3}
                title="Business Growth"
                description="Access a steady stream of new customers and grow your revenue."
                delay={0.1}
              />
              <FeatureCard
                icon={ShieldCheck}
                title="Digital Management"
                description="Manage bookings, quotes, and customer communications in one dashboard."
                delay={0.2}
              />
              <FeatureCard
                icon={DollarSign}
                title="Guaranteed Payments"
                description="Secure payment processing and automated invoicing for every job."
                delay={0.3}
              />
            </div>
          </div>
        </div>
      </div>
    </section>
  );
};



export default FeaturesSection;
