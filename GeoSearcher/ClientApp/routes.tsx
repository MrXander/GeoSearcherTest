import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Search } from './components/Search';

export const routes = <Layout>
    <Route exact path='/' component={ Search } />
    <Route path='/ip' component={ Search } />
    <Route path='/city' component={ Search } />
</Layout>;
