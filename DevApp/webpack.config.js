'use strict';

const webpack = require('webpack');
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;

module.exports = {
  mode: 'development',
  entry: { main: './client/index.js' },
  output: {
    path: __dirname + '/wwwroot/pulse-ui',
    publicPath: '/pulse-ui/'
  },
  resolve: {
    modules: [ 'client', 'node_modules' ]
  },
  externals: {
    react: 'React',
    'react-dom': 'ReactDOM'
  },
  module: {
    rules: [ { test: /\.jsx?$/, use: 'babel-loader', exclude: /node_modules/ } ]
  },
  plugins: [
    new webpack.ContextReplacementPlugin(/moment[/\\]locale$/, /en/)
    //new BundleAnalyzerPlugin()
  ]
};
