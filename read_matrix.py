from __future__ import print_function
  
import csv
import os
import scipy.io
import shutil


matrix_dir = "/Users/vahid/Desktop/10xgenomics/filtered_peak_by_cell_matrix"


peaks = []
with open(os.path.join(matrix_dir, "peaks.bed"), "r") as f:
    for l in f:
        peaks.append(l)


clusters = {}
with open("/Users/vahid/Desktop/10xgenomics/clustering_analysis/clustering/kmedoids_2_clusters/clusters.csv", mode='r') as f:
    reader = csv.reader(f)
    clusters = {rows[0]: rows[1] for rows in reader}


cells_path = os.path.join(matrix_dir, "cells/")
if os.path.exists(cells_path):
    shutil.rmtree(cells_path)
os.makedirs(cells_path)


for k, v in clusters.iteritems():
    cluster_path = os.path.join(cells_path, v)
    if os.path.exists(cluster_path):
        shutil.rmtree(cluster_path)
    os.makedirs(cluster_path)


barcodeFiles = []
with open(os.path.join(matrix_dir, "barcodes.tsv")) as f:
    for l in f:
        barcode = l.strip()
        barcode_file = os.path.join(cells_path, clusters[barcode], "{}.txt".format(barcode))
        open(barcode_file, 'a').close()
        barcodeFiles.append(barcode_file)


mat = scipy.io.mmread(os.path.join(matrix_dir, "matrix.mtx"))
cx = scipy.sparse.coo_matrix(mat)


cx_array = cx.toarray()
for col in range(len(cx_array[0])):
    print("processing cell {} of {}".format(col, len(cx_array[0])), end="\r")
    with open(barcodeFiles[col - 1], "a+") as f:
        for row in range(len(cx_array)):
            value = cx_array[row][col]
            if value > 0:
                f.write(peaks[row])
# for i, j, v in zip(cx.row, cx.col, cx.data):
#     if v > 0:
#         with open(barcodeFiles[j-1], "a+") as f:
#             f.write(peaks[i])
