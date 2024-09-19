import { createRouter, createWebHashHistory } from 'vue-router';
import Login from '../components/Login.vue';
import ReportList from '../components/ReportList.vue';
import authHandler from '../services/authHandler';

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: Login,
  },
  {
    path: '/reports',
    name: 'Reports',
    component: ReportList,
    beforeEnter: (to, from, next) => {
      if (!authHandler.isAuthenticated()) {
        next('/login');
      } else {
        next();
      }
    },
  },
  {
    path: '/',
    redirect: '/login',
  },
];

const router = createRouter({
  history: createWebHashHistory(), // Mudança para hash mode
  routes,
});

export default router;
