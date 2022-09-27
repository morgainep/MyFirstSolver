# -*- coding: utf-8 -*-
"""
Created on Tue Dec 14 21:43:30 2021

@author: Morgaine
"""

import pandas
import numpy
import matplotlib.pyplot as plt

pathA = ''
pathB = ''
pathC = ''

dfA = pandas.read_csv(pathA, sep=';')
dfA.to_numpy()



dfA.boxplot(column='runtime', by='length random walk', showfliers=True, grid=False, showmeans=True, meanline=True)
plt.suptitle('')
plt.title('boxplot S vs runtime with outliers')
plt.ylabel('runtime in ms')
plt.xlabel('length random walk (S)')
means = []
for i in range(1,11):
    mean = dfA.loc[dfA['length random walk'] == i]['runtime'].mean()
    print(mean)
plt.show()

dfB = pandas.read_csv(pathB, sep=';')
dfB.boxplot(column='runtime', by='threshold random walk', showfliers=True, grid=False, return_type='dict', showmeans=True, meanline=True)
for i in range(7,16):
    mean = dfB.loc[dfB['threshold random walk'] == i]['runtime'].mean()
    print(mean)
plt.show()
plt.suptitle('')
plt.title('boxplot P vs runtime with outliers')
plt.ylabel('runtime in ms')
plt.xlabel('height threshold random walk (P)')
plt.show()


dfC = pandas.read_csv(pathC, sep=';')
dfC.to_numpy()
fig = plt.figure(figsize=(8, 3))
ax1 = fig.add_subplot(121, projection='3d')

s = [1, 3, 5, 7, 9, 1, 3, 5, 7, 9, 1, 3, 5, 7, 9, 1, 3, 5, 7, 9, 1, 3, 5, 7, 9]
p = [15, 15, 15, 15, 15, 13, 13, 13, 13, 13, 11, 11, 11, 11, 11, 9, 9, 9, 9, 9, 7, 7, 7, 7, 7]
z = []
for i in range(len(s)):
    mean =dfC.loc[dfC['length random walk'] == s[i]]
    mean = mean.loc[mean['threshold random walk'] == p [i]]
    #print(mean)
    mean = mean['runtime'].mean()
    z.append(mean)
    print("mean for s value:", s[i], "p value:", p[i], "is", mean)

ax1.bar3d(p, s, numpy.zeros_like(p), 2*numpy.ones_like(p), 2*numpy.ones_like(p), z)
plt.title('runtime for different combinations of S and P')
plt.xlabel('height threshold random walk (P)', fontsize=8)
plt.ylabel('length random walk (S)', fontsize=8)
plt.xticks(numpy.arange(7, 16, step=2))
plt.yticks(numpy.arange(1, 11, step=2))
ax1.set_zlabel('runtime in ms', fontsize=8)