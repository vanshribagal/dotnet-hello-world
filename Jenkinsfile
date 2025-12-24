pipeline {
    agent any

    parameters {
        choice(
            name: 'ENVIRONMENT',
            choices: ['UAT', 'PROD'],
            description: 'Select target environment'
        )
        string(
            name: 'IMAGE_TAG',
            defaultValue: 'latest',
            description: 'Docker image tag'
        )
    }

    environment {
        // Docker Image Details
        DOCKERHUB_USERNAME = 'vanshri12'
        IMAGE_NAME = 'dotnet-hello-world'
        FULL_IMAGE = "${DOCKERHUB_USERNAME}/${IMAGE_NAME}:${IMAGE_TAG}"

        // Jenkins Credentials IDs (Configured in Jenkins UI)
        DOCKERHUB_CREDENTIALS = 'dockerhub-creds'
        SSH_CREDENTIALS       = 'app-ec2-ssh'

        // Target EC2 Servers
        UAT_HOST  = 'ubuntu@172.31.20.160'
        PROD_HOST = 'ubuntu@172.31.21.113'
    }

    stages {

        stage('Checkout Code') {
            steps {
                git branch: 'master',
                    url: 'https://github.com/vanshribagal/dotnet-hello-world.git'
            }
        }

        stage('Build Docker Image') {
            steps {
                sh """
                  docker build -t ${FULL_IMAGE} .
                """
            }
        }

        stage('Push Image to Docker Hub') {
            steps {
                withCredentials([
                    usernamePassword(
                        credentialsId: DOCKERHUB_CREDENTIALS,
                        usernameVariable: 'DOCKER_USER',
                        passwordVariable: 'DOCKER_PASS'
                    )
                ]) {
                    sh """
                      echo \$DOCKER_PASS | docker login -u \$DOCKER_USER --password-stdin
                      docker push ${FULL_IMAGE}
                    """
                }
            }
        }

        stage('Deploy to Application EC2') {
            steps {
                script {
                    def TARGET = params.ENVIRONMENT == 'UAT'
                        ? env.UAT_HOST
                        : env.PROD_HOST

                    withCredentials([
                        usernamePassword(
                            credentialsId: DOCKERHUB_CREDENTIALS,
                            usernameVariable: 'DOCKER_USER',
                            passwordVariable: 'DOCKER_PASS'
                        )
                    ]) {
                        sshagent(credentials: [SSH_CREDENTIALS]) {
                            sh """
                            ssh -o StrictHostKeyChecking=no ${TARGET} '
                              echo \$DOCKER_PASS | docker login -u \$DOCKER_USER --password-stdin &&
                              docker pull ${FULL_IMAGE} &&
                              docker stop dotnet-app || true &&
                              docker rm dotnet-app || true &&
                              docker run -d -p 80:80 --name dotnet-app ${FULL_IMAGE}
                            '
                            """
                        }
                    }
                }
            }
        }

        stage('Health Check') {
            steps {
                script {
                    def TARGET = params.ENVIRONMENT == 'UAT'
                        ? env.UAT_HOST
                        : env.PROD_HOST

                    sshagent(credentials: [SSH_CREDENTIALS]) {
                        sh """
                        ssh -o StrictHostKeyChecking=no ${TARGET} \
                        curl -f http://localhost/api/hello
                        """
                    }
                }
            }
        }
    }

    post {
        success {
            echo "✅ Deployment successful to ${params.ENVIRONMENT}"
        }
        failure {
            echo "❌ Deployment failed"
        }
    }
}
